import argparse
import subprocess
from pathlib import Path
from pydub import AudioSegment


def parse_arguments():
    parser = argparse.ArgumentParser(
        description="Convert WAV files to MP3 and normalize all MP3 files in specified asset directories."
    )
    parser.add_argument(
        "-a",
        "--a",
        "-all",
        "--all",
        dest="overwrite",
        action="store_true",
        help="Overwrite existing MP3 files during WAV conversion.",
    )
    parser.add_argument(
        "-f",
        "--f",
        "-file",
        "--file",
        dest="target_file",
        type=str,
        default=None,
        help="Target a specific file by name without extension (e.g., 'DARIKER_EnergyDash').",
    )
    return parser.parse_args()


def process_audio():
    args = parse_arguments()

    # Define target directories relative to current working directory
    base_dir = Path.cwd()
    target_dirs = [
        base_dir.parent / "Assets" / "Music",
        base_dir.parent / "Assets" / "Sounds",
    ]

    # Ensure target directories exist
    for target in target_dirs:
        target.mkdir(parents=True, exist_ok=True)

    # Clean target name: strip any extension user might have accidentally typed
    target_stem = (
        Path(args.target_file).stem.lower() if args.target_file else None
    )

    # =========================================================================
    # STAGE 1: WAV to MP3 Conversion
    # =========================================================================
    print("=" * 60)
    print("STAGE 1: Converting WAV files to MP3")
    print("=" * 60)

    for target_dir in target_dirs:
        print(f"\nScanning: {target_dir.resolve()}")
        wav_files = [
            f for f in target_dir.iterdir() if f.is_file() and f.suffix.lower() == ".wav"
        ]

        # Filter by stem if target_file specified
        if target_stem:
            wav_files = [f for f in wav_files if f.stem.lower() == target_stem]

        if not wav_files:
            print("  └─ No matching .wav files found.")
            continue

        for wav_path in wav_files:
            mp3_path = wav_path.with_suffix(".mp3")

            # Skip existing MP3s unless --all is set
            if mp3_path.exists() and not args.overwrite:
                print(f"  [SKIP] '{wav_path.name}' -> '{mp3_path.name}' already exists.")
                continue

            action = "Overwriting" if mp3_path.exists() else "Converting"
            print(f"  [{action}] '{wav_path.name}' -> '{mp3_path.name}'...")

            try:
                sound = AudioSegment.from_wav(wav_path)
                sound.export(mp3_path, format="mp3", bitrate="192k")
                print(f"    └─ Successfully created '{mp3_path.name}'")
            except Exception as e:
                print(f"    └─ Error converting '{wav_path.name}': {e}")

    # =========================================================================
    # STAGE 2: MP3 Loudness Normalization (ffmpeg-normalize)
    # =========================================================================
    print("\n" + "=" * 60)
    print("STAGE 2: Normalizing MP3 files (EBU R128)")
    print("=" * 60)

    for target_dir in target_dirs:
        print(f"\nScanning: {target_dir.resolve()}")
        mp3_files = [
            f for f in target_dir.iterdir() if f.is_file() and f.suffix.lower() == ".mp3"
        ]

        # Filter by stem if target_file specified
        if target_stem:
            mp3_files = [f for f in mp3_files if f.stem.lower() == target_stem]

        if not mp3_files:
            print("  └─ No matching .mp3 files found to normalize.")
            continue

        for mp3_path in mp3_files:
            print(f"  [Normalizing] '{mp3_path.name}'...")

            cmd = [
                "ffmpeg-normalize",
                "--force",
                str(mp3_path),
                "-o",
                str(mp3_path),
                "-c:a",
                "libmp3lame",
                "-b:a",
                "192k",
            ]

            try:
                result = subprocess.run(
                    cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True
                )
                if result.returncode == 0:
                    print(f"    └─ Done: '{mp3_path.name}' normalized.")
                else:
                    print(f"    └─ Error normalizing '{mp3_path.name}': {result.stderr.strip()}")
            except FileNotFoundError:
                print(
                    "  └─ Error: 'ffmpeg-normalize' executable not found. Run 'pip install ffmpeg-normalize'."
                )
                return

    print("\n" + "=" * 60)
    print("ALL PROCESSING COMPLETE")
    print("=" * 60)


if __name__ == "__main__":
    process_audio()